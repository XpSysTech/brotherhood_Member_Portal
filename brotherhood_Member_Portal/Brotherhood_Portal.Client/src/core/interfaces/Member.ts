export interface Member {
  id?: string;

  displayName: string;
  firstName?: string;
  lastName?: string;

  email: string;

  dateOfBirth: string; // ISO string for forms

  contactNumber: string;
  homeAddress: string;
  homeCity: string;

  imageUrl?: string;
  memberBiography?: string;
  occupation?: string;
  business?: string;

  isActive?: boolean;
  created?: string;
}

export interface MemberByName {
  id: string;
  displayName: string;
  firstName?: string;
  lastName?: string;
}
