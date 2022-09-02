import { Form, Button, Col, Input, Row, Alert } from "antd";
import { Link } from "react-router-dom";
import { React, useState } from "react";
import { UserOutlined } from '@ant-design/icons';
import Config from "../../helpers/Config";
import Recaptcha from "react-google-recaptcha";
import axios from "axios";
import { useHistory } from "react-router";


const ForgotPassword = () => {
	const [form] = Form.useForm();
	const history = useHistory();
	const [alert, setAlert] = useState("");
	const [loading, setLoading] = useState(false);
	const [token, setToken] = useState(null);
	const [state, setState] = useState({
		form: {
			nit: "",
		},
	});
	// Recibimos el token y almacenamos su respuesta en un estado con el hook useState.
	const verifyCallback = function (response) {
		setToken(response);
	};

	const handleChange = (e) => {
		setState({
			form: {
				...state.form,
				[e.target.name]: e.target.value,
			},
		});
	};

	const onFinish = async () => {
		//if (token) {
			const data = {
				email: state.form.username,
			};
			setLoading(true);
			axios
				.post(`${Config.API_URL}/authenticate/forgot-password`, data)
				.finally(() => {
					setAlert("Enlace de reestablecimiento de contraseña enviado al correo electrónico");
					setLoading(false);
                    setTimeout(() => {
						history.push("/token-reset-password")
                    }, 4000)
				});
		//}
	};

	return (
		<>
			<Row style={{ marginTop: "40px" }}>
				<Col span={24} align='center'>
					<Form
						form={form}
						onFinish={onFinish}
						style={{ margin: "0px", maxWidth: "350px" }}
						name='basic'
						initialValues={{ remember: true }}
					>
						{alert && (
							<Alert
								style={{ marginTop: "-30px", marginBottom: "10px" }}
								message={alert}
								type='success'
								showIcon
							/>
						)}
						<h3>¿Olvidaste tu contraseña?</h3>
						<Form.Item
							align='left'
							style={{ marginTop: "20px", width: '100%' }}
							name='username'
							rules={[
								{ required: true, message: "Por favor ingresa tu usuario!" },
							]}
						>
							<Input
								style={{ width: "100%", maxWidth: "300px" }}
								prefix={<UserOutlined />}
								placeholder='Email'
								name='username'
								onChange={handleChange}
							/>
						</Form.Item>
						<Row style={{ marginTop: "10px", paddingLeft: '25px' }}>
							<Recaptcha
								sitekey={Config.GOOGLE_CAPTCHA_KEY}
								render='explicit'
								onChange={verifyCallback}
							/>
						</Row>
						<Row style={{ marginTop: "10px", paddingLeft: '20px' }}>
							<Col span={24}>
								<Form.Item>
									<Button
										type='primary'
										align='left'
										className='btn-primary'
										htmlType='submit'
									>
										{!loading ? "Continuar" : "Cargando..."}
									</Button>
								</Form.Item>
							</Col>
							<Col span={8} offset={8}>
								<Link to='/login' align='right'>
									Iniciar sesión
								</Link>
							</Col>
						</Row>
					</Form>
				</Col>
			</Row>
		</>
	)
}

export default ForgotPassword;