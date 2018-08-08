import { Role } from "./role";

export class User {
  id: number;
  email: string;
  name: string;
  password: string;  
  orderToken: boolean;
  bankAccount: string;
  lockoutEndDateUtc: Date;
  lockoutEnabled: boolean;
  accessFailedCount: number;
  roles: Role[];
}
