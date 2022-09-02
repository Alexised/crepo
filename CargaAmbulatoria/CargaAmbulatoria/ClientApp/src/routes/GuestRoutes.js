import { Link, Route, Switch } from "react-router-dom";
import { Col, Row } from "antd";
import Layout, { Content, Header } from "antd/lib/layout/layout";
import logo from '../static/images/logo_coosalud.png'; // TODO Import Logo
import Login from "../components/Authentication/Login";
import ForgotPassword from "../components/Authentication/ForgotPassword";
import ResetPassword from "../components/Authentication/ResetPassword";
import TokenResetPassword from "../components/Authentication/TokenResetPassword";

import Cookies from "js-cookie";
import axios from "axios";
import { useState } from "react";
import { useHistory } from "react-router";
import Config from "../helpers/Config";
import Helpers from "../helpers/Helpers";

const GuestRoutes = () => {
    const authorization = Cookies.get("access-token");
    const [isLoaded, setIsLoaded] = useState(false);
    const [isLoading, setIsLoading] = useState(false);
    const history = useHistory();
    let load = 0;

    if (!isLoaded && !isLoading) {
        setIsLoading(true);
        axios
            .get(`${Config.API_URL}/authenticate/authorization`, {
                headers: { Authorization: "Bearer " + authorization },
            })
            .then((res) => {
                if (res.data.status) {
                    if (res.data.status !== 401) {
                        Helpers.AUTH_USER = res.data
                        history.push('/app');
                        return;
                    }
                }
            })
            .catch((err) => {
            });
        setIsLoaded(true);
        setIsLoading(false);
    }
    if (isLoaded) {
        return (
            <Layout align="center">
                <Header style={{ background: "#ffffff" }}>
                    <Row>
                        <Col span={24} align="center">
                            <Link to="/login">
                                <img style={{ width: "180px" }} src={logo} alt="Coosalud Logo" />
                            </Link>
                        </Col>
                    </Row>
                </Header>
                <Content style={{ background: "#ffffff" }}>
                    <Row style={{ marginTop: "50px" }}>
                        <Col span={24} align="center">
                            <h3>
                                CARGA AMBULATORIA DE DOCUMENTOS <br /> COOSALUD
                            </h3>
                        </Col>
                    </Row>
                    <Switch>
                        <Route exact path="/login" component={Login} />
                        <Route exact path="/forgot-password" component={ForgotPassword} />
                        <Route exact path="/reset-password/:token" component={ResetPassword} />
                        <Route exact path="/token-reset-password" component={TokenResetPassword} />
                    </Switch>
                </Content>
            </Layout>
        ); 
    }
}
export default GuestRoutes;

