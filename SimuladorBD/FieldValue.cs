namespace SimuladorBD {
    internal class FieldValue {
        public Field FieldType { get; set; }
        public string Value { get; set; }
        public FieldValue(Field fieldType, string value) {
            this.FieldType = fieldType;
            this.Value = value;
            if (!this.FieldType.Validate(value))
                throw new IncorrectFormatException();
        }
        public override string ToString() => this.FieldType.FormatedValue(this.Value);
        
    }
}
