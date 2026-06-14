export interface RegisterUserDto {
  fullName: string;
  email: string;
  phoneNumber: string;
  username: string;
  password: string;
  confirmPassword: string;
  referralCode?: string;
}

export interface LoginUserDto {
  emailOrPhone: string;
  password: string;
}

export interface UserDto {
  userId: string;
  fullName: string;
  username: string;
  email: string;
  phoneNumber: string;
  roleName: string;
  referralCode: string;
  isEmailVerified: boolean;
  isPhoneVerified: boolean;
  status: string;
  createdDate: string;
}

export interface TokenResponseDto {
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
  tokenType: string;
}

export interface AuthResponseDto {
  success: boolean;
  message: string;
  user?: UserDto;
  accessToken?: string;
  refreshToken?: string;
}
