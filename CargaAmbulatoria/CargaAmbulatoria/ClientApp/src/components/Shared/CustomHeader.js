import { Button, Col, Row, Card } from "antd";
import Layout from "antd/lib/layout/layout";
import Cookies from "js-cookie";
import { useEffect, useState } from "react";
import { useHistory } from "react-router";
import { Link } from "react-router-dom";
import logo from '../../static/images/logo_coosalud.png';
import './CustomHeader.css';
import { PoweroffOutlined, SettingOutlined } from "@ant-design/icons";

const CustomHeader = ({ username, role }) => {

    const [width, setWidth] = useState(window.innerWidth);
    const history = useHistory();
    const [isEdit, setIsEdit] = useState(false);
    const [userId, setUserId] = useState('');
    const [isModalAddUserVisible, setModalAddUserVisible] = useState(false);

    const Logout = () => {
        Cookies.remove("access-token");
        history.push("/login")
    }

    useEffect(() => {
        window.addEventListener("resize", () => setWidth(window.innerWidth));

    }, [username]);

    return (
        <Layout className="prop" style={{ background: "#ffffff", marginBottom: "10px" }} align="center">
            <Row style={{ background: "#ffffff" }}>
                <div className="bannerContainer">
                    <img src={logo} alt="Logo" width='100%' />
                </div>
                <Row align="right" style={{ padding: "10px 20px 0px 20px", width: '100%' }} gutter={{ xs: 8, sm: 16, md: 24, lg: 32 }}>
                    <Col className="gutter-row" span={18} offset={18} align="right">
                        <Row>
                            <Col span={6} align="center">
                                <h5>Hola, {username}!</h5>
                            </Col>
                            <Col span={6} align="left">
                                <Button type="primary" className="btn-primary" onClick={Logout} style={{ marginLeft: "5px" }}>
                                    <PoweroffOutlined />
                                </Button>
                            </Col>
                        </Row>
                    </Col>
                </Row>
                {role !== "Evaluator" ?
                    <>
                        <Row align="right" style={{ paddingTop: '10px', width: '100%' }} >
                            <Card style={{ height: "75px", textAlign: "left", boxShadow: "5px 8px 24px 5px rgba(208, 216, 243, 0.6)", marginTop: "10px", width: '100%' }} align="center">
                                <Row align="right" style={{ width: '100%' }} >
                                    {role == "Agent" ?
                                        <Col span={10}>
                                            <Link to="/app/documents" style={{ color: '#00000' }}>
                                                <h3 style={{ display: "inline-block" }}>Carga de Documentos</h3>
                                            </Link>
                                        </Col>
                                        : ""
                                    }
                                    {role == "Admin" ?
                                        <>
                                            <Col span={6}>
                                                <Link to="/app/users" style={{ color: '#00000' }}>
                                                    <h3 style={{ display: "inline-block", marginLeft: "50px" }}>Usuarios</h3>
                                                </Link>
                                            </Col>
                                        </>
                                        : ""
                                    }
                                </Row>
                            </Card>
                        </Row>
                    </>
                    :
                    ""
                }
            </Row>
        </Layout>

    );
};

export default CustomHeader;
