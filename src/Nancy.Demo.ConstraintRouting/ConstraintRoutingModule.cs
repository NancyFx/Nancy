namespace Nancy.Demo.ModelBinding
{
    public class ConstraintRoutingModule : NancyModule
    {
        public ConstraintRoutingModule()
        {
            Get["/"] = _ => View["Index"];

            Get["/intConstraint/{value:int}"] = _ => "Value " + _.value + " is an int.";

            Get["/decimalConstraint/{value:decimal}"] = _ => "Value " + _.value + " is an decimal.";

            Get["/guidConstraint/{value:guid}"] = _ => "Value " + _.value + " is an guid.";

            Get["/boolConstraint/{value:bool}"] = _ => "Value " + _.value + " is an boolean.";

            Get["/alphaConstraint/{value:alpha}"] = _ => "Value " + _.value + " is only letters.";

            Get["/datetimeConstraint/{value:datetime}"] = _ => "Value " + _.value + " is a date time.";

            Get["/minConstraint/{value:min(4)}"] = _ => "Value " + _.value + " is an int and more than 4.";

            Get["/maxConstraint/{value:max(6)}"] = _ => "Value " + _.value + " is an int and less 6.";

            Get["/rangeConstraint/{value:range(10, 20)}"] = _ => "Value " + _.value + "  is an int between 10 and 20.";

            Get["/minlengthConstraint/{value:minlength(4)}"] = _ => "Value " + _.value + " is a string with length greater than 4.";

            Get["/maxlengthConstraint/{value:maxlength(10)}"] = _ => "Value " + _.value + " is a string with length less than 10.";

            Get["/lengthConstraint/{value:length(1, 20)}"] = _ => "Value " + _.value + " is an less than 1 more than 20.";
        }
    }
}