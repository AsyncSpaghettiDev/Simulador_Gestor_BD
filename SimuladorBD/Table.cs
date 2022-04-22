using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace SimuladorBD {
    internal class Table {
        public List<Field> Structure { get; private set; }
        public List<Record> Records { get; private set; }
        public string Name { get => Path.GetFileNameWithoutExtension(this.StructPath).ToUpper(); }
        private string StructPath { get; set; }
        private string DataPath { get; set; }
        private int LastFieldIndex = 0;
        public Table(string structPath, string dataPath) {
            this.Structure = new List<Field>();
            this.Records = new List<Record>();
            this.StructPath = structPath;
            this.DataPath = dataPath;
        }
        public Table(string rawData, string structPath, string dataPath) {
            this.Structure = new List<Field>();
            this.Records = new List<Record>();
            this.StructPath = structPath;
            this.DataPath = dataPath;
            AddFields(rawData);
        }
        public void LoadStructure(string compressedData) {
            // Uncompress the data and creates an array
            string[] formatedData = compressedData.ToUpper().Split(',');

            IdentifyFieldType(1, formatedData, formatedData.Length);
            AsignFieldsRange();
        }
        private void AddFields(string rawData) {
            string[] formatedData = Stuff.FormatedData(rawData, 3);

            IdentifyFieldType(1, formatedData, formatedData.Length);
            AsignFieldsRange();

            WriteStruct();
        }
        public void AddNewFields(string rawData) {

            string[] formatedData = Stuff.FormatedData(rawData, 3);

            IdentifyFieldType(1, formatedData, formatedData.Length);
            this.LastFieldIndex = 0;
            AsignFieldsRange();

            foreach (Record record in this.Records)
                record.UpdateStructure(this.Structure);
            WriteStruct();
            WriteRecords();
        }
        private void IdentifyFieldType(int typeIndex, string[] originalArray, int maxIndex) {
            Field dataField = null;
            if (this.Structure.Exists(field => field.NameField.ToUpper() == originalArray[typeIndex - 1].ToUpper()))
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
            this.Structure.Add(dataField);
            if (typeIndex < maxIndex)
                IdentifyFieldType(typeIndex, originalArray, maxIndex);
        }
        private void AsignFieldsRange(int index = 0) {
            this.Structure[index].Start = this.LastFieldIndex;
            this.LastFieldIndex += this.Structure[index].FieldLength;
            index++;
            if (index < this.Structure.Count)
                AsignFieldsRange(index);
        }
        private void WriteStruct() {
            StreamWriter textOut = null;
            try {
                textOut = new StreamWriter(new FileStream(this.StructPath, FileMode.Create, FileAccess.Write));
                foreach (Field dataStruct in this.Structure)
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
            Field fieldToDelete = this.Structure.Find(field => field.NameField == fieldName);
            this.Structure.Remove(fieldToDelete);
            this.LastFieldIndex = 0;
            AsignFieldsRange();
            foreach (Record record in this.Records)
                record.DeleteField(fieldToDelete, this.Structure);
            WriteStruct();
            WriteRecords();
        }
        private Field FindField(string fieldName) => this.Structure.Find(field => field.NameField == fieldName);
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
            this.Records.Add(new Record(this.Structure, fieldValues));
            WriteRecords();
        }
        public int Delete(string compressedCondition) {
            string[] uncompressedCondition = compressedCondition.Split('=');
            string fieldName = uncompressedCondition[0].Trim().ToUpper();
            string fieldValue = uncompressedCondition[1].Trim();
            int deletedRecords = this.Records.RemoveAll(record => record.Match(fieldName, fieldValue));
            
            WriteRecords();
            return deletedRecords;
        }
        public int Update(string compressedCondition, string[] valuesToUpdate) {
            string[] uncompressedCondition = compressedCondition.Split('=');
            string fieldName = uncompressedCondition[0].Trim().ToUpper();
            string fieldValue = uncompressedCondition[1].Trim();

            List<Record> recordsToUpdate = this.Records.FindAll(record => record.Match(fieldName, fieldValue));
            foreach (Record recordToUpdate in recordsToUpdate)
                recordToUpdate.UpdateRecord(valuesToUpdate);
            WriteRecords();
            return recordsToUpdate.Count;
        }
        public void ListAll() {
            foreach (Record record in this.Records)
                Console.WriteLine(record);
        }
        public void ListAllWhere( string compressedCondition ) {
            string[] uncompressedCondition = compressedCondition.Split('=');
            string fieldName = uncompressedCondition[ 0 ].Trim().ToUpper();
            string fieldValue = uncompressedCondition[ 1 ].Trim();
            List<Record> filteredRecords = this.Records.FindAll(record => record.Match(fieldName, fieldValue));
            foreach (Record record in filteredRecords)
                Console.WriteLine(record);
        }
        public void ListFields( string compressedFields ) {
            string[] uncompressedFields = compressedFields.Split(',');
            foreach (Record record in this.Records)
                Console.WriteLine(record.GetFields(uncompressedFields));
        }
        public void ListFieldsWhere( string compressedFields, string compressedCondition ) {
            string[] uncompressedFields = compressedFields.Split(',');
            foreach (Record record in this.Records) {
                string filteredRecord = record.GetFieldsWhere(uncompressedFields, compressedCondition);
                if (filteredRecord != null)
                    Console.WriteLine(filteredRecord);
                else
                    continue;
            }
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
        public void LoadData(string[] compressedData) {
            foreach (string compressedRecord in compressedData) {
                char[] uncompressedRecord = compressedRecord.ToCharArray();
                List<FieldValue> fieldList = new List<FieldValue>();

                foreach (Field structuralField in this.Structure) {
                    string fieldValue = new string(
                        uncompressedRecord.
                    Skip(structuralField.Start).
                    Take(structuralField.FieldLength).
                    ToArray()
                    );
                    fieldList.Add(new FieldValue(structuralField, fieldValue));
                }
                this.Records.Add(new Record(this.Structure, fieldList));
            }
        }
        override public string ToString() {
            return $"{this.Name}\n\u0020\u0020{string.Join("\n\u0020\u0020", this.Structure)}";
        }
        public static string GetRawStructure(string fullPath) => string.Join(",", File.ReadAllLines(fullPath));
        public static string[] GetRawData(string fullPath) => File.ReadAllLines(fullPath);
    }
}