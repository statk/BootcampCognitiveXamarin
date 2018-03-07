using System;
namespace EmployeeDirectory
{
	public class Person
	{
		public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Title { get; set; }
		public string PhotoUrl { get; set; }
        public string Name => FirstName + " " + LastName;
        
	}
}