import { Form, Button, Col, Input, Row, Modal } from "antd";
import { React, useEffect, useState } from "react";
import { useParams} from 'react-router-dom'
import Cookies from "js-cookie";
import { useHistory } from "react-router";
import Config from "../../helpers/Config";
import axios from "axios";

const ResetPassword = () => {
	const [form] = Form.useForm();
	const { token = null } = useParams();
	const history = useHistory();
	const [loading, setLoading] = useState(false);
	const [mounted, setMounted] = useState(true);
	const [inSession, setInSession] = useState(false);
	const [isNew, setIsNew] = useState(0);
	const authorization = Cookies.get("access-token");
	const [state, setState] = useState({
		form: {
			oldPassword: "",
			password: "",
			confirm: "",
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

	const onFinish = async () => {
		const data = {
			token,
			newPassword: state.form.password,
			oldPassword: state.form.oldPassword,
			UserId: ''
		};
		setLoading(true);
		const redirect = inSession ? "/app" : "/login";
		axios
			.post(`${Config.API_URL}/authenticate/reset-password`, data, {
				headers: { Authorization: "Bearer " + authorization },
			})
			.then(({ data }) => {
				if (data.status === 401) {
					Modal.error({
						okText: "Aceptar",
						title: data.message,
						onOk() {},
					});
				} else {
					if (data.success) {
						Modal.success({
							okText: "Aceptar",
							title: "Contraseña actualizada",
							onOk() {
								history.push(redirect);
							},
						});

                    } else {
						Modal.error({
							okText: "Aceptar",
							title: "Ha ocurrido un error, por favor intente de nuevo",
							onOk() {
								history.push(redirect);
							},
						});
                    }

					
				}
			})
			.catch((err) => {
				Modal.error({
					okText: "Aceptar",
					title: "Ha ocurrido un error, por favor intente de nuevo",
					onOk() {
						history.push(redirect);
					},
				});
			})
			.finally(() => {
				setLoading(false);
			});
	};

	useEffect(() => {

		if (!mounted) {
			if (token) {
				axios
					.get(`${Config.API_URL}/authenticate/validate-token?token=${token}`)
					.then((res) => {
						if (res.data) {
							if (res.data.success === true) {
								setMounted(true);
							} else {
								history.push("/login");
								return null;
							}
						}
					})
					.catch(() => {
						history.push("/login");
					});
			} else {
				axios
					.get(`${Config.API_URL}/authenticate/authorization`, {
						headers: { Authorization: "Bearer " + authorization },
					})
					.then((res) => {
						if (res.data) {
							if (res.data.status === 401) {
								history.push("/login");
								return null;
							} else {
								setMounted(true);
								setInSession(true);
								setIsNew(res.data.isNew);
							}
						}
					})
					.catch(() => {
						history.push("/login");
					});
			}
		}
	}, [authorization, history, mounted, token]);

	if (!mounted) return <></>;

	return (
		<>
			<Row style={{ marginTop: "40px" }}>
				<Col span={24} align='center'>
					<Form
						form={form}
						onFinish={onFinish}
						style={{ margin: "0px", width: "230px" }}
						name='basic'
						initialValues={{ remember: true }}
					>
						<h3>Restablecer Contraseña</h3>
						{inSession && isNew === 0 && (
							<Form.Item
								style={{ marginTop: "20px" }}
								name='oldPassword'
								rules={[
									{
										required: true,
										message: "Por favor ingresa tu contraseña actual!",
									},
								]}
								hasFeedback
							>
								<Input.Password
									name='oldPassword'
									onChange={handleChange}
									value={state.form.oldPassword}
									placeholder={"Contraseña Actual"}
								/>
							</Form.Item>
						)}
						<Form.Item
							style={{ marginTop: /*inSession && isNew === 0 ? "0px" :*/ "20px" }}
							name='password'
							rules={[
								{
									required: true,
									message: "Por favor ingresa tu nueva contraseña!",
								},
							]}
							hasFeedback
						>
							<Input.Password
								name='password'
								onChange={handleChange}
								value={state.form.password}
								placeholder={"Nueva Contraseña"}
							/>
						</Form.Item>
						<Form.Item
							name='confirm'
							rules={[
								{
									required: true,
									message: "Por favor ingresa la contraseña a confirmar!",
								},
								({ getFieldValue }) => ({
									validator(_, value) {
										if (!value || getFieldValue("password") === value) {
											return Promise.resolve();
										}

										return Promise.reject(
											new Error("Las contraseñas no coinciden")
										);
									},
								}),
							]}
							dependencies={["password"]}
							hasFeedback
						>
							<Input.Password
								name='confirm'
								onChange={handleChange}
								value={state.form.confirm}
								placeholder={"Confirmar Contraseña"}
							/>
						</Form.Item>

						<Form.Item style={{ marginTop: "10px" }}>
							<Button
								type='primary'
								htmlType='submit'
								className='btn-primary'
							>
								{!loading ? "Guardar" : "Cargando..."}
							</Button>
						</Form.Item>
					</Form>
				</Col>
			</Row>
		</>
	);
};

export default ResetPassword;