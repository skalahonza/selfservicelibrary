namespace SelfServiceLibrary.BL.DTO.Card
{
    public class AddCardDTO
    {
#pragma warning disable CS8618 // Enforced by validation
        public string Number { get; set; }
        public string Name { get; set; }
#pragma warning restore CS8618 // Enforced by validation
        public string? Pin { get; set; }
        public string? PinConfirmation { get; set; }
    }
}
