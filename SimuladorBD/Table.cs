using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SimuladorBD {
    internal class Table {
        public List<Field> Fields { get; private set; }
        public List<Record> Records { get; private set; }
        public string Name { get => Path.GetFileNameWithoutExtension(this.StructPath).ToUpper(); }
        private string StructPath { get; set; }
        private string DataPath { get; set; }
        public Table(string structPath, string dataPath) {
            this.Fields = new List<Field>();
            this.Records = new List<Record>();
            this.StructPath = structPath;
            this.DataPath = dataPath;
        }
        public Table(string rawData, string structPath, string dataPath) {
            this.Fields = new List<Field>();
            this.Records = new List<Record>();
            this.StructPath = structPath;
            this.DataPath = dataPath;
            AddFields(rawData);
        }
        public void LoadFields(string compressedData) {
            // Uncompress the data and creates an array
            string[] formatedData = compressedData.ToUpper().Split(',');

            IdentifyFieldType(1, formatedData, formatedData.Length);
        }
        public void AddFields(string rawData) {
            string[] formatedData = Stuff.FormatedData(rawData, 3);

            IdentifyFieldType(1, formatedData, formatedData.Length);

            WriteStruct();
        }
        private void IdentifyFieldType(int typeIndex, string[] originalArray, int maxIndex) {
            Field dataField = null;
            if (this.Fields.Exists(field => field.NameField.ToUpper() == originalArray[typeIndex - 1].ToUpper()))
                throw new DuplicatedNamefieldException();
            switch (originalArray[typeIndex]) {
                case "ENTERO":
                    dataField = new IntegerField(originalArray[typeIndex - 1], int.Parse(originalArray[typeIndex + 1]));
                    typeIndex += 3;
                    break;

                case "CARACTER":
                    dataField = new CharacterField(originalArray[typeIndex - 1], int.Parse(originalArray[typeIndex + 1]));
                    typeIndex += 3;
                    break;

                case "DECIMAL":
                    dataField = new DecimalField(originalArray[typeIndex - 1], int.Parse(originalArray[typeIndex + 1]), int.Parse(originalArray[typeIndex + 2]));
                    typeIndex += 4;
                    break;

                case "FECHA":
                    dataField = new DateField(originalArray[typeIndex - 1]);
                    typeIndex += 2;
                    break;
            }
            this.Fields.Add(dataField);
            if (typeIndex < maxIndex)
                IdentifyFieldType(typeIndex, originalArray, maxIndex);
        }
        private void WriteStruct() {
            StreamWriter textOut = null;
            try {
                textOut = new StreamWriter(new FileStream(this.StructPath, FileMode.Create, FileAccess.Write));
                foreach (Field dataStruct in this.Fields)
                    textOut.WriteLine(dataStruct.ToString());
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
            finally {
                if (textOut != null)
                    textOut.Close();
            }
        }
        public void DeleteField(string fieldName) {
            this.Fields.Remove(this.Fields.Find(field => field.NameField == fieldName));
            WriteStruct();
        }
        private Field FindField(string fieldName) => this.Fields.Find(field => field.NameField == fieldName);
        public void Insert(string[] values) {
            List<FieldValue> fieldValues = new List<FieldValue>();
            foreach (string compressedData in values) {
                string[] uncompressedData = compressedData.Split('=');
                IsDuplicated(values, uncompressedData[0]);
                fieldValues.Add(new FieldValue(
                    FindField(uncompressedData[0].Trim()),
                    uncompressedData[1].Trim())
                    );
            }
            this.Records.Add(new Record(this.Fields, fieldValues));
            WriteRecords();
        }
        private bool IsDuplicated(string[] values, string toCompare) {
            bool isOnce = false;
            foreach (string item in values) {
                string[] uncompressedData = item.Split('=');

                if (uncompressedData[0].ToUpper().Trim() == toCompare.ToUpper().Trim()) {
                    if (isOnce)
                        throw new DuplicatedFieldInQueryException();
                    isOnce = true;
                }
            }
            return false;
        }

        private void WriteRecords() {
            StreamWriter textOut = null;
            try {
                textOut = new StreamWriter(new FileStream(this.DataPath, FileMode.Create, FileAccess.Write));
                foreach (Record record in this.Records)
                    textOut.WriteLine(record.ToString());
            }
            catch (Exception e) {
                Console.WriteLine(e.ToString());
                Console.ReadKey();
            }
            finally {
                if (textOut != null)
                    textOut.Close();
            }
        }
        override public string ToString() {
            return $"{this.Name}\n\u0020\u0020{string.Join("\n\u0020\u0020", this.Fields)}";
        }
        public static string GetRawData(string fullPath) => string.Join(",", File.ReadAllLines(fullPath));

    }
}