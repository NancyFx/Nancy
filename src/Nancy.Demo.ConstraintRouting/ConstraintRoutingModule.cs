namespace Nancy.Demo.ModelBinding
{
    public class ConstraintRoutingModule : NancyModule
    {
        public ConstraintRoutingModule()
        {
            Get["/"] = _ => View["Index"];

            Get["/intConstraint/{value:int}"] = _ => "Value " + _.value + " is an integer.";

            Get["/decimalConstraint/{value:decimal}"] = _ => "Value " + _.value + " is a decimal.";

            Get["/guidConstraint/{value:guid}"] = _ => "Value " + _.value + " is a guid.";

            Get["/boolConstraint/{value:bool}"] = _ => "Value " + _.value + " is a boolean.";

            Get["/alphaConstraint/{value:alpha}"] = _ => "Value " + _.value + " is only letters.";

            Get["/datetimeConstraint/{value:datetime}"] = _ => "Value " + _.value + " is a date time.";

            Get["/customDatetimeConstraint/{value:datetime(yyyy-MM-dd)}"] = _ => "Value " + _.value + " is a date time with format 'yyyy-MM-dd'.";

            Get["/minConstraint/{value:min(4)}"] = _ => "Value " + _.value + " is an integer greater than 4.";

            Get["/maxConstraint/{value:max(6)}"] = _ => "Value " + _.value + " is an integer less than 6.";

            Get["/rangeConstraint/{value:range(10, 20)}"] = _ => "Value " + _.value + "  is an integer between 10 and 20.";

            Get["/minlengthConstraint/{value:minlength(4)}"] = _ => "Value " + _.value + " is a string with length greater than 4.";

            Get["/maxlengthConstraint/{value:maxlength(10)}"] = _ => "Value " + _.value + " is a string with length less than 10.";

            Get["/lengthConstraint/{value:length(1, 20)}"] = _ => "Value " + _.value + " is a string with length between 1 and 20.";

            Get["/emailConstraint/{value:email}"] = _ => "Value " + _.value + " is an e-mail address (according to @jchannon).";
        }
    }
}