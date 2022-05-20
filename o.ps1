$uri = "http://localhost:5138/api/"
(irm "${uri}options/monitored" && irm "${uri}options/snapshot" && irm "${uri}options") | ft