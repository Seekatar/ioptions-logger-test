$uri = "http://localhost:5138/api/"
(irm "${uri}config" && irm "${uri}config/section" ) | ft