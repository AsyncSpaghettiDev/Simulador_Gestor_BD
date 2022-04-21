namespace SimuladorBD {
    internal class FieldValue {
        public Field FieldType { get; set; }
        private string _value;
        public string Value {
            get { return _value; }
            set {
                if (!this.FieldType.Validate(value))
                    throw new IncorrectFormatException();
                _value = value;
            }
        }
        public FieldValue(Field fieldType, string value) {
            this.FieldType = fieldType;
            this.Value = value;
        }
        public override string ToString() => this.FieldType.FormatedValue(this.Value);
        
    }
}
