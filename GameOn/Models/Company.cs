using System;
using System.Collections.Generic;
using System.Text;

namespace GameOn.Models
{
    public class Company
    {
		private int companyId;

		public int CompanyId
		{
			get { return companyId; }
			set
			{
				if (value < 0)
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

        private string domain = string.Empty;

        public string Domain
        {
            get { return domain; }
            set
            {
                if (string.IsNullOrWhiteSpace(value) || value.Contains('@'))
                {
                    throw new ArgumentException("The domain must be like 'company.com', without @ !");
                }
                domain = value.Trim().ToLower();
            }
        }


        private List<Department> departments;

        public IReadOnlyList<Department> Departments
        {
            get { return departments; }
        }


        public Company(int companyId, string name, string domain)
		{
			CompanyId = companyId;
			Name = name;
			Domain = domain;
			departments = new List<Department>();
		}
	}
}
