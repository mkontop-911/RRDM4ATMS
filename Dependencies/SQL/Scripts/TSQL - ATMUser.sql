USE ATMS
EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser

USE ATMS_Journals
EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser

-- USE ATM_Journals_Diebold
-- EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser

-- USE RRDM_AGENT
-- EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser

USE RRDM_Reconciliation
EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser

USE RRDM_Reconciliation_ITMX
EXEC sp_change_Users_login UPDATE_ONE, ATMUser, ATMUser
