A package for implimantion of property notification

sample code:

    public class Person : PropertyNotificationSupport
    {
        private int age;

        public int Age
        {
            get => age;
            set
            {
                if (value == age) return;
                age = value;
                OnPropertyChanged();
            }
        }

        private bool citizen;

        public bool Citizen
        {
            get => citizen;
            set
            {
                if (value == citizen) return;
                citizen = value;
                OnPropertyChanged();
            }
        }

        private bool canVote;

        public bool CanVote
        {
            get => canVote;
            set
            {
                if (value == canVote) return;
                canVote = value;
                OnPropertyChanged();
            }
        }

        public Person()
        {
            AddProvider(nameof(CanVote), () => Citizen && Age >= 16);
        }
    }

    public class Employee : PropertyNotificationSupport
    {
        private int age;

        public int Age
        {
            get => age;
            set
            {
                if (value == age) return; // critical
                age = value;
                OnPropertyChanged();
            }
        }
    }

    public class Program
    {
        public static void Main(string[] args)
        {
            var person = new Person()
            {
                Age = 32
            };
            var employee = new Employee()
            {
                Age = 32
            };
            
            person.BindProperty(employee, () => person.Age, () => employee.Age);

            person.Age = 33;
            Console.WriteLine(employee.Age);
            // don't need to write employee.Age = 33 
        }
    }
