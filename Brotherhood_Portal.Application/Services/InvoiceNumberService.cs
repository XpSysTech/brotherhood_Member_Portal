using Brotherhood_Portal.Application.Interfaces;
using Brotherhood_Portal.Domain.Entities;

namespace Brotherhood_Portal.Application.Services
{
    public class InvoiceNumberService
    {
        /*
            - Coordinates database access
            - Applies business rules
            - Maintains stateful sequencing
            - Produces a business artifact (invoice number)
         */

        private readonly IInvoiceSequenceRepository _invoiceSequence;

        public InvoiceNumberService(IInvoiceSequenceRepository invoiceSequence)
        {
            _invoiceSequence = invoiceSequence;
        }

        #region Generate Next Invoice Number
        public async Task<string> GenerateAsync(string memberId)
        {
            // Get the current year
            var year = DateTime.UtcNow.Year;

            // Retrieve or create the sequence record
            var sequence = await _invoiceSequence.GetAsync(memberId, year);

            // This is initialized when it is a new year for the member
            if (sequence == null)
            {
                // Initialize a new sequence for the member and year
                sequence = new MemberInvoiceSequence
                {
                    MemberId = memberId,
                    Year = year,
                    CurrentNumber = 1
                };

                await _invoiceSequence.AddAsync(sequence);
            }
            else
            {
                // Else Increment the existing sequence number
                sequence.CurrentNumber++;
            }

            await _invoiceSequence.SaveChangesAsync();

            // Format: MEM001-2026-0001
            return $"{memberId[..6].ToUpper()}-{year}-{sequence.CurrentNumber:D4}";
        }
        #endregion
    }
}
