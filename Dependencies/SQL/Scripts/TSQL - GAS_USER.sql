USE ATMS
EXEC sp_change_Users_login UPDATE_ONE, GAS_USER, GAS_USER

USE ATMS_Journals
EXEC sp_change_Users_login UPDATE_ONE, GAS_USER, GAS_USER

USE RRDM_AGENT
EXEC sp_change_Users_login UPDATE_ONE, GAS_USER, GAS_USER

USE RRDM_Reconciliation
EXEC sp_change_Users_login UPDATE_ONE, GAS_USER, GAS_USER
