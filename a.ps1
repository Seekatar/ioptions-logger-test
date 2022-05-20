$uri = "http://localhost:5138/api/"
(irm "${uri}config" && irm "${uri}config/section" && irm "${uri}options/monitored" && irm "${uri}options/snapshot" && irm "${uri}options") | ft