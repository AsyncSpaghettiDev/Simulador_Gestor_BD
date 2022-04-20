using System;

namespace SimuladorBD {
    internal class HandledException : Exception {
        public HandledException() : base() { }
        public HandledException(string message) : base(message) { }
    }
    internal class InstructionNotFoundException : HandledException {
        public InstructionNotFoundException() :
            base("La instrucción dada no está soportada, corrige tu sintaxis e intente otra vez. ") { }
    }
    internal class ExitProgramException : HandledException {
        public ExitProgramException() : base() { }
    }
    internal class ReturnToMainException : HandledException {
        public ReturnToMainException() : base() { }
    }
    internal class DatabaseAlreadyExistsException : HandledException {
        public DatabaseAlreadyExistsException() : base("La de base de datos ya existe, prueba con otro nombre. ") { }
    }
    internal class DatabaseNotExistsException : HandledException {
        public DatabaseNotExistsException() : base("La de base de datos no existe, prueba con otro nombre. ") { }
    }
    internal class TableNotExistsException : HandledException {
        public TableNotExistsException() : base("La tabla no existe en la base de datos, prueba con otro nombre. ") { }
    }
    internal class TableAlreadyExistsException : HandledException {
        public TableAlreadyExistsException() : base("La tabla ya existe en la base de datos, favor de ingresar otro nombre. ") { }
    }
    internal class DuplicatedNamefieldException : HandledException {
        public DuplicatedNamefieldException() : base("Nombre de campo repetido, prueba con otro nombre. ") { }
    }
    internal class IncorrectFormatException : HandledException {
        public IncorrectFormatException() : base("El dato no tiene el formato correcto, intentar otra vez. ") { }
    }
}
