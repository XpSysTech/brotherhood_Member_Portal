export interface UserDto {
  id: string;
  displayName: string;
  email: string;
  imageUrl?: string | null;
  token: string;
}

export interface LoginCredentialsDto {
    email: string;
    password: string;
}   

export interface RegisterCredentialsDto {
    displayName: string;
    email: string;
    password: string;
}

export interface JwtPayload {
  nameid: string;
  email: string;
  role?: string[] | string;
}

export interface UpdateProfileDto {
  firstName: string;
  lastName: string;
  email: string;
  contactNumber: string;
  homeAddress: string;
  homeCity: string;
  dateOfBirth: string;
  memberBiography: string;
  occupation: string;
  business: string;
  imageUrl: string;
}

export const createEmptyProfile = (): UpdateProfileDto => ({
  firstName: '',
  lastName: '',
  email: '',
  contactNumber: '',
  homeAddress: '',
  homeCity: '',
  dateOfBirth: '',
  memberBiography: '',
  occupation: '',
  business: '',
  imageUrl: ''
});

export interface ResetPasswordDto {
  userId: string;
  newPassword: string;
}