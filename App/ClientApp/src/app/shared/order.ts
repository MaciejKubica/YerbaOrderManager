export class Order {
  id: number;
  orderDate: Date;
  created: Date;
  userMadeBy: CompactUser;
  madeBy: number;
  userExecutedBy: CompactUser;
  executedBy: number;
  totalQuantity: number;
  totalCost: number;
  isClosed: boolean;
  isPaid: boolean;
  items: OrderItem[];
}

export class OrderItem {
  id: number;
  yerbaId: number;
  quantity: number;
  orderId: number;
  isPaid: boolean;
  cost: number;
  userId: number;
  userDetails: CompactUser;
}

export class CompactUser {
  id: number;
  name: string;
  email: string;
}
