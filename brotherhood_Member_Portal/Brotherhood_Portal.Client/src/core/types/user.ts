// User object type definition
export type User = {
    id: string;
    displayName: string;
    email: string;
    token: string;
    imageUrl?: string;
}

export type LoginCredentials = {
    email: string;
    password: string;
}   

export type RegisterCredentials = {
    displayName: string;
    email: string;
    password: string;
}