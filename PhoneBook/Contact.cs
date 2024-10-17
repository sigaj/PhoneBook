using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PhoneBook
{
    internal class Contact
    {
        public int ID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }

        public Contact(int id, string first_name, string last_name, string phone_number, string address)
        {
            this.ID = id;
            this.FirstName = first_name;
            this.LastName = last_name;
            this.PhoneNumber = phone_number;
            this.Address = address;
        }
    }
}
