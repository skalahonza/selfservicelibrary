using System.Threading.Tasks;
using System.Web;

using Microsoft.AspNetCore.Mvc;

using SelfServiceLibrary.BL.Interfaces;

namespace SelfServiceLibrary.API.Controllers
{
    public enum NfcType
    {
        Book,
        UserNoPin,
        UserYesPin
    }

    public record DiscoveryResponse(NfcType Type, string? RedirectUrl);

    public class NfcController : BaseController
    {
        private readonly IBookService _bookService;
        private readonly ICardAuthenticator _cardAuthenticator;

        public NfcController(IBookService bookService, ICardAuthenticator cardAuthenticator)
        {
            _bookService = bookService;
            _cardAuthenticator = cardAuthenticator;
        }

        /// <summary>
        /// Discover NFC tag meaning
        /// </summary>
        /// <param name="serNumNFC"></param>
        /// <returns></returns>
        [HttpGet("{serNumNFC}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<DiscoveryResponse>> Get(string serNumNFC)
        {
            // try to find a book
            var book = await _bookService.GetByNFC(serNumNFC);

            if (book is not null)
                return Ok(new DiscoveryResponse(NfcType.Book, $"/books/{book.DepartmentNumber}"));

            // try to find an access card
            if(await _cardAuthenticator.Exists(serNumNFC))
            {
                if (await _cardAuthenticator.HasPin(serNumNFC))
                {
                    return Ok(new DiscoveryResponse(NfcType.UserYesPin, null));
                }
                else
                {
                    var token = await _cardAuthenticator.GetToken(serNumNFC, null);
                    return Ok(new DiscoveryResponse(NfcType.UserNoPin, $"/login?card={serNumNFC}&token={HttpUtility.UrlEncode(token)}"));
                }
            }

            return NotFound();
        }
    }
}
