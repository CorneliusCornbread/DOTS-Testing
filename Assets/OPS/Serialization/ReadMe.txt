Abstract:
--------------------------------------------------------------------

Thank you for your decision to use OrangePearSoftwares Serialization.

You will find three demos explaining all you need to know!

Please start with the 'Mesh' demo.
Then the 'Compress and Encrypt' demo.
After this the 'Complex Save Load' demo.
And last but not least the 'Async' demo.

Start:
--------------------------------------------------------------------

If you do not want to have a look at the demo, here a quickstart guide:

//SAVE
FileStream stream = new FileStream("Save.ser", FileMode.Create);
OPS.Serialization.IO.Serializer.SerializeToStream(stream, object);
stream.Close();

//LOAD
FileStream stream = new FileStream("Save.ser", FileMode.Open);
MyObject object = OPS.Serialization.IO.Serializer.DeSerializeFromStream<MyObject>(stream);
stream.Close();

//Attributes
[SerializeAbleClass]
[SerializeAbleField(0)]
[ClassInheritance(typeof(Employee), 0)]

//HOW TO USE
The Attributes, like 'SerializeAbleClass', can be found in the namespace 'OPS.Serialization.Attributes'.
The Attribute 'SerializeAbleClass' has to be added to a class to mark it as serializeable.
But only adding this Attribute will not save any fields.
You have now to mark the fields you want to serialize with the Attribute 'SerializeAbleField'.
The field type has to be a primitive or a Unity primitive or a class marked by you with 'SerializeAbleClass'.
The 'SerializeAbleField' has a parameter called Index. This Index has to be a unique integer, used as key
to identify the field. Start at best at zero.

To allow inheritance you have to add the Attribute 'ClassInheritance' on the base class.
For example: The 'Teacher' class is serializable and inherites from the serializeable 'Employee' class.
So the 'Employee' class needs an 'ClassInheritance' to 'Teacher'.

//
[SerializeAbleClass]
[ClassInheritance(typeof(Teacher), 0)]
class Employee
{
	...
	[SerializeAbleField(0)]
	public String Department;
	...
}

As you can see you need to link to the type and you have to enter an increasing Index like you know from 
the 'SerializeAbleField' Attribute. These Indexes are independent from the ones used for the fields. So
they can share the same integer values you used for the fields. And vice versa.