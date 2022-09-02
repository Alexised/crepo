import { Form, Button, Col, Input, Row, Alert } from "antd";
import axios from "axios";
import Cookies from "js-cookie";
import { Link } from "react-router-dom";
import { useState } from "react";
import { useHistory } from "react-router";
import Recaptcha from "react-google-recaptcha";
import Config from "../../helpers/Config";
import { UserOutlined, LockOutlined } from '@ant-design/icons';

const Login = () => {
	const API_URL = "https://localhost:7252/api"
	const [form] = Form.useForm();
	const history = useHistory();
	const [token, setToken] = useState(null);
	const [error, setError] = useState("");
	const [loading, setLoading] = useState(false);
	const [state, setState] = useState({
		form: {
			username: "",
			password: "",
		},
	});
	const handleChange = (e) => {
		setState({
			form: {
				...state.form,
				[e.target.name]: e.target.value,
			},
		});
	};

	// Recibimos el token y almacenamos su respuesta en un estado con el hook useState.
	const verifyCallback = function (response) {
		setToken(response);
	};
	const onFinish = async () => {
		// if (token) {
		const data = {
			email: state.form.email,
			password: state.form.password,
		};
		setLoading(true);
		axios
			.post(`${Config.API_URL}/authenticate/login`, data)
			//.post(`${API_URL}/authenticate/login`, data)
			.then((res) => {
				if (res.data) {
					if (res.data.Success === false) {
						setError(res.data.Error);
						Cookies.remove("access-token");
					} else {
						Cookies.set("access-token", res.data.token);
						Config.AUTH_USER = res.data;
						history.push("/app/");
					}
				}
			})
			.catch((error) => {
				if(error.response?.data)
				{
					setError(error.response.data.error);
				}
				else
				{
					setError("Usuario y/o contraseña incorrecta");
				}
				Cookies.remove("access-token");
			})
			.finally(() => setLoading(false));
		// }
	};

	return (
		<>
			<Row style={{ marginTop: "40px" }}>
				<Col span={24} align='center'>
					<h3>INICIO DE SESIÓN</h3>
					<Form
						form={form}
						onFinish={onFinish}
						style={{ margin: "0px", maxWidth: "350px" }}
						name='basic'
						initialValues={{ remember: true }}
					>
						{error !== "" && (
							<Alert
								style={{ marginTop: "-30px", marginBottom: "10px" }}
								message={error}
								type='error'
								showIcon
							/>
						)}
						<Row style={{ paddingTop: "10px" }}>
							<Form.Item
								align='left'
								style={{ marginTop: "20px", width: '100%' }}
								name='email'
								rules={[
									{ required: true, message: "Por favor ingresa tu usuario!" },
								]}
							>
								<Input
									style={{ width: "100%", maxWidth: "300px" }}
									prefix={<UserOutlined />}
									placeholder='Usuario'
									name='email'
									onChange={handleChange}
								/>
							</Form.Item>
						</Row>
						<Row style={{ paddingTop: "10px" }}>
							<Form.Item
								name='password'
								style={{ width: '100%' }}
								rules={[
									{
										required: true,
										message: "Por favor ingresa tu contraseña!",
									},
								]}
							>
								<Input.Password
									visibilityToggle={false}
									style={{ width: "100%", maxWidth: "300px" }}
									placeholder='Contraseña'
									name='password'
									prefix={<LockOutlined />}
									onChange={handleChange}
								/>
							</Form.Item>
						</Row>
						<Row style={{ paddingTop: "10px", paddingLeft: '20px' }}>
							<Recaptcha
								textAlign='center'
								sitekey={Config.GOOGLE_CAPTCHA_KEY}
								render='explicit'
								size="normal"
								onChange={verifyCallback}
							/>
							<div style={{ paddingTop: '10px', width: '50%' }}>
								<Link to='/forgot-password'>¿Olvidaste tu contraseña?</Link>
							</div>
						</Row>
						<Form.Item style={{ marginTop: "10px" }}>
							<Button
								disabled={process.env.NODE_ENV !== 'development' && token == null}
								type='primary'
								className='btn-primary'
								htmlType='submit'
							>
								{!loading ? "Iniciar Sesión" : "Cargando..."}
							</Button>
						</Form.Item>
					</Form>
				</Col>
			</Row>
		</>
	);
};

export default Login;
