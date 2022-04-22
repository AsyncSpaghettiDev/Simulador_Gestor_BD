namespace SimuladorBD {
    internal abstract class Field {
        public string NameField { get; protected set; }
        public abstract int FieldLength { get; }
        public int Start { get; set; }
        public int End => this.Start + this.FieldLength - 1;
        public Field(string nameField) {
            this.NameField = nameField;
        }
        public abstract string FormatedValue(string toFormat);
        public abstract bool Validate(string toValidate);
    }
    internal class IntegerField : Field {
        public int IntLength { get; private set; }
        public override int FieldLength => this.IntLength;
        public IntegerField(string nameField, int length) : base(nameField) {
            this.IntLength = length;
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat)) throw new IncorrectFormatException();
            return toFormat.PadLeft(this.IntLength, '\u0020');
        }
        public override bool Validate(string toValidate) => toValidate.Length <= this.IntLength && toValidate.Trim() == string.Empty || long.TryParse(toValidate, out _);
        public override string ToString() {
            return $"{this.NameField},entero,{this.IntLength}";
        }

    }
    internal class CharacterField : Field {
        public int CharacterLength { get; private set; }
        public override int FieldLength => this.CharacterLength;
        public CharacterField(string nameField, int length) : base(nameField) {
            this.CharacterLength = length;
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat))
                throw new IncorrectFormatException();
            return toFormat.PadRight(this.CharacterLength, '\u0020');
        }
        public override bool Validate(string toValidate) => toValidate.Length <= this.CharacterLength;
        public override string ToString() {
            return $"{this.NameField},caracter,{this.CharacterLength}";
        }
    }
    internal class DecimalField : Field {
        public int IntLength { get; private set; }
        public int DecimalLength { get; private set; }
        public override int FieldLength => this.IntLength + this.DecimalLength;
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
            && toValidate.Trim() == string.Empty || long.TryParse(toValidate, out _);
        public override string ToString() {
            return $"{this.NameField},decimal,{this.IntLength},{this.DecimalLength}";
        }
    }
    internal class DateField : Field {
        public override int FieldLength => 8;
        public DateField(string nameField) : base(nameField) { }
        public override bool Validate(string toValidate) {
            if (toValidate.Trim() == string.Empty)
                return true;
            char[] uncompressedDate = toValidate.ToCharArray();
            string year = new string(uncompressedDate, 0, 4);
            string month = new string(uncompressedDate, 4, 2);
            string day = new string(uncompressedDate, 6, 2);
            string newDate = $"{year}-{month}-{day}";
            return toValidate.Length <= 8 && System.DateTime.TryParse(newDate, out _);
        }
        public override string FormatedValue(string toFormat) {
            if (!Validate(toFormat))
                throw new IncorrectFormatException();
            return toFormat.PadLeft(8, '\u0020');
        }
        public override string ToString() {
            return $"{this.NameField},fecha";
        }
    }

}
