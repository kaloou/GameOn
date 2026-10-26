using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    internal class Company
    {
		private int companyId;

		public int CompanyId
		{
			get { return companyId; }
			set
			{
				if (value <= 0)
				{
					throw new ArgumentOutOfRangeException("The companyId can't be negative or equal to zero !");
				}
				companyId = value;
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
					throw new ArgumentException("The name of the company can't be empty !");
				}
				name = value;
			}
		}

        private List<Department> departments;

        public IReadOnlyList<Department> Departments
        {
            get { return departments; }
        }


        public Company(int companyId, string name)
		{
			CompanyId = companyId;
			Name = name;
			departments = new List<Department>();
		}
	}
}
