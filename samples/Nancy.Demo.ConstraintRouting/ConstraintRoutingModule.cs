namespace Nancy.Demo.ModelBinding
{
    public class ConstraintRoutingModule : NancyModule
    {
        public ConstraintRoutingModule()
        {
            Get("/", args => View["Index"]);

            Get("/intConstraint/{value:int}", args => "Value " + args.value + " is an integer.");

            Get("/decimalConstraint/{value:decimal}", args => "Value " + args.value + " is a decimal.");

            Get("/guidConstraint/{value:guid}", args => "Value " + args.value + " is a guid.");

            Get("/boolConstraint/{value:bool}", args => "Value " + args.value + " is a boolean.");

            Get("/alphaConstraint/{value:alpha}", args => "Value " + args.value + " is only letters.");

            Get("/datetimeConstraint/{value:datetime}", args => "Value " + args.value + " is a date time.");

            Get("/customDatetimeConstraint/{value:datetime(yyyy-MM-dd)}", args => "Value " + args.value + " is a date time with format 'yyyy-MM-dd'.");

            Get("/minConstraint/{value:min(4)}", args => "Value " + args.value + " is an integer greater than 4.");

            Get("/maxConstraint/{value:max(6)}", args => "Value " + args.value + " is an integer less than 6.");

            Get("/rangeConstraint/{value:range(10, 20)}", args => "Value " + args.value + "  is an integer between 10 and 20.");

            Get("/minlengthConstraint/{value:minlength(4)}", args => "Value " + args.value + " is a string with length greater than 4.");

            Get("/maxlengthConstraint/{value:maxlength(10)}", args => "Value " + args.value + " is a string with length less than 10.");

            Get("/lengthConstraint/{value:length(1, 20)}", args => "Value " + args.value + " is a string with length between 1 and 20.");

            Get("/versionConstraint/{value:version}", args => "Value " + args.value + " is a version number.");

            Get("/emailConstraint/{value:email}", args => "Value " + args.value + " is an e-mail address (according to @jchannon).");
        }
    }
}