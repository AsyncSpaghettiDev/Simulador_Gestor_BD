using System;
using System.IO;
using System.Collections.Generic;

namespace SimuladorBD {
    internal class Database {
        public string Name { get; private set; }
        public string Location { get; private set; }
        public List<Table> Tables { get; private set; }
        readonly private string FullPath;
        public Database(string name, string path) {
            this.Name = name;
            this.Location = path;
            this.FullPath = this.Location + '\u005C';
            LoadTables();
        }
        public void CreateTable(string tableName, string rawData) {
            string DataPath = this.FullPath + tableName + ".dat";
            string StructPath = this.FullPath + tableName + ".est";
            if (File.Exists(DataPath))
                throw new TableAlreadyExistsException();
            File.Create(DataPath).Close();
            File.Create(StructPath).Close();
            Table newTable = new Table(rawData, StructPath, DataPath);
            this.Tables.Add(newTable);
            Console.Write("Tabla creada correctamente...");
            Console.ReadKey();
        }
        public void DeleteTable(string tableName) {
            string DataPath = this.FullPath + tableName + ".dat";
            string StructPath = this.FullPath + tableName + ".est";
            File.Delete(DataPath);
            File.Delete(StructPath);
            Console.Write($"Tabla {tableName} eliminada correctamente...");
            Console.ReadKey();
            LoadTables();
        }
        private void LoadTables() {
            this.Tables = new List<Table>();
            foreach (string file in Directory.GetFiles(this.FullPath, "*.est")) {
                string tableName = Path.GetFileNameWithoutExtension(file).ToUpper();
                string DataPath = this.FullPath + tableName + ".dat";
                string StructPath = this.FullPath + tableName + ".est";
                Table newTable = new Table(StructPath, DataPath);
                newTable.LoadStructure(Table.GetRawStructure(StructPath));
                newTable.LoadData(Table.GetRawData(DataPath));
                this.Tables.Add(newTable);
            }
        }
        public void ShowTables() {
            foreach (Table table in this.Tables)
                Console.WriteLine(table);
            Console.ReadKey();
        }
        private Table FindTable(string tableName) => this.Tables.Find(table => table.Name.ToUpper() == tableName.ToUpper()) ?? throw new TableNotExistsException();
        
        public void DeleteField(string fullQuery, string tableName) {
            string fieldName = Stuff.FormatedData(fullQuery, 3)[0];
            FindTable(tableName).DeleteField(fieldName);
            LoadTables();
            Console.Write($"Campo {fieldName} eliminado correctamente...");
            Console.ReadKey();
        }
        public void AddField(string fullQuery, string tableName) {
            string fieldName = Stuff.FormatedData(fullQuery, 3)[0];
            FindTable(tableName).AddNewFields(fullQuery);
            Console.Write($"Campo {fieldName} agregado correctamente...");
            Console.ReadKey();
        }
        public void Insert(string fullQuery, string tableName) {
            string[] values = Stuff.FormatedData(fullQuery, 3, false);
            FindTable(tableName).Insert(values);
            Console.WriteLine("Nuevo registro agregado.");
            Console.ReadKey();
        }
    }
}
