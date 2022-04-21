using System.Linq;
using System.Collections.Generic;
using System;

namespace SimuladorBD {
    internal class Record {
        public List<FieldValue> Values { get; private set; }
        public List<Field> RecordStruct { get; set; }
        public string FullPath { get; private set; }
        public Record(List<Field> recordStruct, List<FieldValue> values) {
            this.RecordStruct = recordStruct;
            this.Values = values;
            SortValues();
        }
        private void SortValues() {
            Values.OrderBy(d => {
                int index = RecordStruct.IndexOf(d.FieldType);
                return index == -1 ? int.MaxValue : index;
            });
        }
        public void DeleteField(Field fieldToDelete, List<Field> newRecordStruct) {
            foreach(FieldValue fieldValue in this.Values) {
                if (fieldValue.FieldType.NameField == fieldToDelete.NameField) {
                    this.Values.Remove(fieldValue);
                    break;
                }
            }
            this.RecordStruct = newRecordStruct;
        }

        public bool Match(string fieldName, string fieldValue) =>
            this.Values.Exists(value => value.FieldType.NameField == fieldName && value.Value == fieldValue);
        public void UpdateRecord(string[] valuesToUpdate) {
            foreach(string compressedNewValue in valuesToUpdate) {
                string[] uncompressedNewValues = compressedNewValue.Split('=');
                string fieldNewName = uncompressedNewValues[0].Trim().ToUpper();
                string fieldNewValue = uncompressedNewValues[1].Trim();
                FieldValue fieldToUpdate = this.Values.Find(valueField => valueField.FieldType.NameField == fieldNewName);
                if (fieldToUpdate != null)
                    fieldToUpdate.Value = fieldNewValue;
                else {
                    Field fieldType = this.RecordStruct.Find(valueType => valueType.NameField == fieldNewName);
                    this.Values.Add(new FieldValue(fieldType, fieldNewValue));
                    SortValues();
                }
            }
        }
        public void UpdateStructure(List<Field> newRecordStruct) => this.RecordStruct = newRecordStruct;
        override public string ToString() {
            SortValues();
            List<FieldValue> recordToWrite = new List<FieldValue>();
            int currentValueIndex = 0;
            for(int i = 0; i < this.RecordStruct.Count; i++) {
                if(currentValueIndex < this.Values.Count && this.RecordStruct[i].NameField == this.Values[currentValueIndex].FieldType.NameField) {
                    recordToWrite.Add(this.Values[currentValueIndex]);
                    currentValueIndex++;
                    continue;
                }
                recordToWrite.Add(new FieldValue(this.RecordStruct[i], string.Empty));
            }
            return string.Join(string.Empty, recordToWrite);
        }
    }
}
