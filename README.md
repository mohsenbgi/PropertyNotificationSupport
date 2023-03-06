
# PropertyNotificationSupport

A package for implementing property notification.



### Add a provider

in this feature value of a property will calculte when some peroperty change.

For example:
```
public class Person : PropertyNotificationSupport
{
    public Person()
    {
        AddProvider(nameof(CanVote), () => Citizen && Age >= 16);
    }

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
}
```
So when "Citizen" or "Age" change, "CanVote" value will calculate.


### Bind properties of two classes

Employee:
```
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
```

Person:
```
public class Person : PropertyNotificationSupport
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
```

So we can do this:
```
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
```

### Use custom event handlres
```
public class Person : PropertyNotificationSupport
{
    public Person()
    {
        PropertyChanged += NewHandler;
    }

    private void NewHandler(object sender, EventArgs e)
    {
        // hanle
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            PropertyChanged -= NewHandler;
        }
        else
        {
            try
            {
                PropertyChanged -= NewHandler;
            }
            catch (Exception e)
            {
                // nothing
            }
        }

        base.Dispose(disposing);    
    }

    ~Person()
    {
        Dispose(false);
    }
}
```
