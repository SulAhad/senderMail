using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace mailSender
{
    public class email
    {
        private int id;
        private string tabNumber;
        private string name;
        private string emailUser;

        public email()
        {
        }

        public email(int id, string tabNumber, string name, string emailUser)
        {
            this.id = id;
            this.tabNumber = tabNumber;
            this.name = name;
            this.emailUser = emailUser;
        }

        public int Id { get => id; set => id = value; }
        public string TabNumber { get => tabNumber; set => tabNumber = value; }
        public string Name { get => name; set => name = value; }
        public string EmailUser { get => emailUser; set => emailUser = value; }

        internal static void EnsureCreated()
        {
            throw new NotImplementedException();
        }
    }
}
