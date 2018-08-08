import { CompactUser } from "./order";

export class Role {
  id: number;
  name: string;  
}

export class UserRole {
  userId: number;
  roleId: number;

  roleDetails: Role;
  userDetails: CompactUser
}
