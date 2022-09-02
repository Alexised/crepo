import { Route, Switch, useHistory, useRouteMatch } from "react-router";
import { useState } from "react";
import Cookies from "js-cookie";
import axios from "axios";
import CustomHeader from "../components/Shared/CustomHeader";
import UserManagement from "../components/User/UserManagement";
import Helpers from "../helpers/Helpers";
import Config from "../helpers/Config";
import DocumentManagement from "../components/Document/DocumentManagement";

const AuthenticatedRoutes = () => {
	const API_URL = "https://localhost:7252/api"

	const { path } = useRouteMatch();
	const [isLoaded, setIsLoaded] = useState(false);
	const [username, setUsername] = useState("");
	const [role, setRole] = useState("");
	const [user, setUser] = useState([]);
	const history = useHistory();
	const authorization = Cookies.get("access-token");
	if (!isLoaded) {
		axios
			.get(`${Config.API_URL}/authenticate/authorization`, {
			//.get(`${API_URL}/authenticate/authorization`, {
				headers: { Authorization: "Bearer " + authorization },
			})
			.then((res) => {
				if (res.data) {
					if (res.data.status === 401) {
						history.push("/login");
						return null;
					} else if (res.data.IsNew === 1) {
						history.push("/reset-password");
					} else {
						setIsLoaded(true);
						setUsername(res.data.user);
						setRole(res.data.role);
						setUser(res.data.userData);

						Helpers.AUTH_USER = res.data;
						return;
					}
				}
			})
			.catch((err) => {
				history.push("/login");
			});
	}

	if (isLoaded) {
		return (
			<>
				<CustomHeader username={username} role={role} />

				<Switch>
					{role === "Admin" && <Route exact path={`${path}/users`} component={UserManagement} />}
					{role === "Admin" && <Route exact path={`${path}/`} component={UserManagement} />}
					{role !== "Admin" &&
						<Route exact path={`${path}/documents`}>
						<DocumentManagement role={role} user={user} />
						</Route>
					}
					{role !== "Admin"  &&
						<Route exact path={`${path}/`}>
						<DocumentManagement role={role} user={user} />
						</Route>
					}
				</Switch>
			</>
		);
	}

	return null;
};

export default AuthenticatedRoutes;
