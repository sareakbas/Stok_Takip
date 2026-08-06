export interface LoginRequest {
    email: string;
    password: string;
  }
  
export interface LoginResponse {
    data: string;
    message: string;
    success: boolean;
  }

export interface RegisterRequest {
    fullName: string;
    email: string;
    password: string;
  }
  
export interface RegisterResponse {
    data: boolean;
    message: string;
    success: boolean;
  }