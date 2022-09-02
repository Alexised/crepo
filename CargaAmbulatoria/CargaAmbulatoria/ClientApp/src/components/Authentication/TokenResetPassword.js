import { Form, Button, Col, Input, Row, Alert } from "antd";
import { React, useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { useHistory } from "react-router";

const TokenResetPassword = () => {
	const [form] = Form.useForm();
	const [alert, setAlert] = useState("");
	const history = useHistory();

	const [state, setState] = useState({
		form: {
			token: "",
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
		history.push(`/reset-password/${state.form.token}`)
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
						<h3>Ingresa tu código</h3>
						<Form.Item
							align='left'
							style={{ marginTop: "20px", width: '100%' }}
							name='token'
							rules={[
								{ required: true, message: "¡Por favor ingresa tu código!" },
							]}
						>
							<Input
								style={{ width: "100%", maxWidth: "300px" }}
								placeholder='Código'
								name='token'
								onChange={handleChange}
							/>
						</Form.Item>

						<Row style={{ marginTop: "10px", paddingLeft: '20px' }}>
							<Col span={24}>
								<Form.Item>
									<Button
										type='primary'
										align='left'
										className='btn-primary'
										htmlType='submit'
									//disabled={token == null}
									>
										{/*{!loading ? "Continuar" : "Cargando..."}*/}
										{"Continuar"}
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
	);
}

export default TokenResetPassword;
