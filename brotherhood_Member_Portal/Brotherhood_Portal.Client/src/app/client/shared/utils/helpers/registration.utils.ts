// registration.utils.ts

/**
 * Formats a standard member registration message
 */
export function buildRegistrationMessage(): string {
  const message = `
Hello,

I would like to request registration as a member.

My first name is:
My last name is:
My email address is:

Thank you.
  `;

  return encodeURIComponent(message.trim());
}

/**
 * Creates a WhatsApp deep link for registration requests
 */
export function buildWhatsAppRegistrationLink(
  phoneNumber: string
): string {
  const cleanPhone = phoneNumber.replace('+', '');
  const message = buildRegistrationMessage();

  return `https://wa.me/${cleanPhone}?text=${message}`;
}

/**
 * Creates an SMS deep link for registration requests
 */
export function buildSmsRegistrationLink(
  phoneNumber: string
): string {
  const message = buildRegistrationMessage();
  return `sms:${phoneNumber}?body=${message}`;
}
