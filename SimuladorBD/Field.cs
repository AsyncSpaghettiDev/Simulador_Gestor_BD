namespace SimuladorBD {
    internal abstract class Field {
        public string NameField { get; protected set; }
        public int Start { get; set; }
        public int End { get; set; }
        public Field(string nameField) {
            this.NameField = nameField;
        }
        public abstract string FormatedValue(string toFormat);
        public abstract bool Validate(string toValidate);
    }
    internal class IntegerField : Field {
        public int IntLength { get; private set; }
        public IntegerField(string nameField, int length) : base(nameField) {
            this.IntLength = length;
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat)) throw new IncorrectFormatException();
            return toFormat.PadLeft(this.IntLength, '\u0020');
        }
        public override bool Validate(string toValidate) => toValidate.Length <= this.IntLength && int.TryParse(toValidate, out _);
        public override string ToString() {
            return $"{this.NameField},entero,{this.IntLength}";
        }

    }
    internal class CharacterField : Field {
        public int CharacterLength { get; private set; }
        public CharacterField(string nameField, int length) : base(nameField) {
            this.CharacterLength = length;
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat))
                throw new IncorrectFormatException();
            return toFormat.PadLeft(this.CharacterLength, '\u0020');
        }
        public override bool Validate(string toValidate) => toValidate.Length <= this.CharacterLength;
        public override string ToString() {
            return $"{this.NameField},caracter,{this.CharacterLength}";
        }
    }
    internal class DecimalField : Field {
        public int IntLength { get; private set; }
        public int DecimalLength { get; private set; }
        public DecimalField(string nameField, int intLength, int decimalLength) : base(nameField) {
            this.IntLength = intLength;
            this.DecimalLength = decimalLength;
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat))
                throw new IncorrectFormatException();
            return toFormat.PadLeft(this.IntLength + this.DecimalLength, '\u0020');
        }
        public override bool Validate(string toValidate) =>
            toValidate.Length <= this.DecimalLength + this.IntLength
            && int.TryParse(toValidate, out _);
        public override string ToString() {
            return $"{this.NameField},decimal,{this.IntLength},{this.DecimalLength}";
        }
    }
    internal class DateField : Field {
        public DateField(string nameField) : base(nameField) { }
        public override bool Validate(string toValidate) => toValidate.Length <= 8;
        public override string FormatedValue(string toFormat) {
            return toFormat;
        }
        public override string ToString() {
            return $"{this.NameField},fecha";
        }
    }

}
