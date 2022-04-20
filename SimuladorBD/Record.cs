using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimuladorBD {
    internal class Record {
        public Field FieldType { get; set; }
        public string Value { get; set; }
        public Record(Field fieldType, string value) {
            this.FieldType = fieldType;
            this.Value = value;
            if (!this.FieldType.Validate(value))
                throw new IncorrectFormatException();
        }
        public override string ToString() => this.FieldType.FormatedValue(this.Value);
        
    }
}
