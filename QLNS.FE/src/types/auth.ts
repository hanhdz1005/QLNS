export interface LoginDto {
  email: string;
  password: string;
}

export interface RegisterDto {
  displayName: string;
  userName: string;
  email: string;
  password: string;
}

export interface TokenDto {
  accessToken: string;
  refreshToken: string;
}

export interface AuthResultDto {
  accessToken: string;
  refreshToken: string;
}

export interface GetCurrentUserDto {
  id: string;
  displayName: string;
  role: string;
}

export interface UserInfoDto {
  id: string;
  userName: string;
  email: string;
  displayName: string;
  role: string;
}

export interface IdentityRole {
  id: string;
  name: string;
  normalizedName?: string;
}

export interface AssignRoleDto {
  email: string;
  role: string;
}

export interface AssignClaimDto {
  email: string;
  claimValue: string;
}
