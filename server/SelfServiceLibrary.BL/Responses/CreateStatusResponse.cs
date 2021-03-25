
using FuncSharp;

namespace SelfServiceLibrary.BL.Responses
{
    public class StatusCreated { }
    public class StatusAlreadyExisted { }

    public class CreateStatusResponse : Coproduct2<StatusCreated, StatusAlreadyExisted>
    {
        public CreateStatusResponse(StatusCreated firstValue) : base(firstValue)
        {
        }

        public CreateStatusResponse(StatusAlreadyExisted secondValue) : base(secondValue)
        {
        }

        public CreateStatusResponse(ICoproduct2<StatusCreated, StatusAlreadyExisted> source) : base(source)
        {
        }

        protected CreateStatusResponse(int discriminator, object value) : base(discriminator, value)
        {
        }
    }
}
