using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Text;

namespace GameOn.Models
{
    internal class Department
    {
		private int departmentId;

		public int DepartmentId
		{
			get { return departmentId; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("The departmentId can't be negative or equal to zero !");
                }
                departmentId = value;
            }
        }

        private string name;

        public string Name
        {
            get { return name; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("The name of the department can't be empty !");
                }
                name = value;
            }
        }

        private Company company;

        public Company Company
        {
            get { return company; }
        }


        public Department(int departmentId, string name, Company company)
        {
            DepartmentId = departmentId;
            Name = name;
            this.company = company;
        }

    }
}
