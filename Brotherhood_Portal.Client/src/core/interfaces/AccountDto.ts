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