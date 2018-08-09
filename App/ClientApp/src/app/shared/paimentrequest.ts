import { OrderItem, CompactUser } from "./order";

export class PaimentRequests {
  id: number;
  orderItem: number;
  userId: number;

  userDetails: CompactUser;
  orderItemDetails: OrderItem;
}
