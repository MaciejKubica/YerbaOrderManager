export class CheckPassword {
  UserId: number;
  Password: string;

  constructor(userId, password) {
    this.UserId = userId;
    this.Password = password;
  }
}
