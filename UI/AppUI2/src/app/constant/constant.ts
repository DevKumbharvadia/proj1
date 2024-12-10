export const Constant = {
    API_METHOD: {
        ADMIN: {
            REWRITE_ROLES: '/api/Admin/RewriteRoles',
            BLACKLIST_USER: '/api/Admin/BlacklistUser',
            GET_WHITELISTED_USERS : '/api/Admin/GetAllWhiteListedUserInfo'
        },
        AUTH: {
            LOGOUT: '/api/Auth/Logout',
            REGISTER: '/api/Auth/Register',
            LOGIN: '/api/Auth/Login',
            REFRESH_TOKEN: '/api/Auth/refresh-token',
        },
        BUYER_INFO: {
            GET_BUYER_INFO: '/api/BuyerInfo/GetBuyerInfo',
            ADD_BUYER_INFO: '/api/BuyerInfo/AddBuyerInfo',
            UPDATE_BUYER_INFO: '/api/BuyerInfo/UpdateBuyerInfo',
            BUYER_INFO_EXIST: '/api/BuyerInfo/BuyerInfoExist',
        },
        PRODUCT: {
            GET_ALL: '/api/Product/GetAllProducts',
            GET_BY_ID: '/api/Product/GetProductById',
            GET_PRODUCT_DETAILS_BY_ID : '/api/Product/GetProductDetailsById',
            ADD: '/api/Product/AddProduct',
            UPDATE: '/api/Product/UpdateProduct',
            DELETE: '/api/Product/DeleteProduct',
        },
        PRODUCT_STOCK: {
            GET_ALL_STOCK_LOG: '/api/ProductStock/GetAllStockLogs',
            GET_STOCK_LOG_BY_ID: '/api/ProductStock/GetStockLogByProductId',
            ADD_PRODUCT_STOCK_LOG: '/api/ProductStock/AddProductStockLog',
            UPDATE_PRODUCT_STOCK_LOG: '/api/ProductStock/UpdateProductStockLog',
            DELETE_PRODUCT_STOCK_LOG: '/api/ProductStock/DeleteProductStockLog',
        },
        ROLE: {
            GET_ALL: '/api/Role/GetAllRoles',
            GET_BY_ID: '/api/Role/GetUserRolesByUserID',
            ADD: '/api/Role/AddRole',
            UPDATE: '/api/Role/UpdateRole',
            DELETE: '/api/Role/DeleteRole',
        },
        SELLER: {
            ALL_SALES_DATA: '/api/Seller/AllSalesData',
            SALES_DATA_BY_ID: '/api/Seller/SalesDataByID',
            SHIP_ORDER: '/api/Seller/ShipOrders',
        },
        SORTED_PRODUCT: {
            GET_SORTED_PRODUCTS: '/api/SortedProduct/GetSortedProduct',
            GET_BY_SELLER_ID: '/api/SortedProduct/getSortedProductsBySellerId'
        },
        TRANSACTION: {
            GET_ALL: '/api/Transaction/getAllTransaction',
            GET_BY_ID: '/api/Transaction/getAllTransactionById',
            MAKE_PURCHASE: '/api/Transaction/MakePurchase',
            GET_TRANSACTION_HISTORY: '/api/Transaction/getTransactionHistory',
            GET_TRANSACTION_HISTORY_BY_USER_ID: '/api/Transaction/getTransactionHistoryByUserId',
            GET_UNSHIPED_PRODUCTS: '/api/Transaction/getAllUnshippedTransaction',
            SHIP_PRODUCT: '/api/Transaction/ShipItems'
        },
        USER: {
            ADD_USER: '/api/User/AddUser',
            GET_ALL: '/api/User/GetAllUsers',
            GET_BY_ID: '/api/User/GetAllUsers',
            UPDATE_USER: '/api/User/UpdateUser',
            DELETE_USER: '/api/User/DeleteUser'
        },
        USER_ACTION: {
            GET_ALL: '/api/UserAction/GetUserActions',
            GET_BY_AUDIT_ID: '/api/UserAction/GetUserActionsByAuditId',
            ADD_ACTION: '/api/UserAction/AddAction',
        },
        USER_AUDIT: {
            GET_ALL: '/api/UserAudit/GetAllAudits',
            GET_BY_USER_ID: '/api/UserAudit/GetAuditsByUserID'
        },
        USER_INFO: {
            GET_ALL: '/api/UserInfo/GetAllUserInfo',
            GET_BY_USER_ID: '/api/UserInfo/GetUserInfoById'
        },
        USER_ROLE: {
            ASSIGN_ROLE: '/api/UserRole/AssignRole',
            ASSIGN_ROLES: '/api/UserRole/AssignRoles'
        }
    }
}
