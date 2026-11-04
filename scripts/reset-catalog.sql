-- Clears the catalog and everything that points at it (orders, payments, carts, wishlists, reviews)
-- so the application can seed the current showcase catalog again on its next start.
-- Users, roles, addresses and coupons are kept.
--
-- Run it on the database, then restart the site. The seeder only fills empty tables.
-- Take a backup first if the data matters: this cannot be undone.

SET XACT_ABORT ON;
BEGIN TRANSACTION;

DELETE FROM Reviews;
DELETE FROM CouponUsages;
DELETE FROM PaymentTransactions;
DELETE FROM Payments;
DELETE FROM Shipments;
DELETE FROM OrderStatusHistories;
DELETE FROM OrderShippingAddresses;
DELETE FROM OrderItems;
DELETE FROM Orders;

DELETE FROM CartItems;
DELETE FROM WishlistItems;
DELETE FROM InventoryTransactions;

DELETE FROM ProductVariantOptionValues;
DELETE FROM ProductVariants;
DELETE FROM ProductOptionValues;
DELETE FROM ProductOptions;
DELETE FROM ProductImages;
DELETE FROM Products;

DELETE FROM Categories WHERE ParentCategoryId IS NOT NULL;
DELETE FROM Categories;
DELETE FROM Brands;

COMMIT TRANSACTION;
