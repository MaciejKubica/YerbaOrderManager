import { Role } from "./role";

export class User {
  id: number;
  email: string;
  name: string;
  password: string;  
  orderTokenLocker: boolean;
  bankAccount: string;
  lockoutEndDateUtc: Date;
  lockoutEnabled: boolean;
  accessFailedCount: number;
  isDeleted: boolean;
  roles: Role[];
}
