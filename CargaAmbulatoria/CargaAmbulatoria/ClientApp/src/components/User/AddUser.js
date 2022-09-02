import { Modal, Row, Col, Form, Select, Input, Button } from 'antd'
import { React, useEffect, useState } from 'react'
import axios from "axios";
import Config from '../../helpers/Config'
import Cookies from "js-cookie";



const { Option } = Select;

export const AddUser = ({ isModalAddUserVisible, setModalAddUserVisible, loading, setLoading }) => {
    const API_URL = "https://localhost:7252/api"


    const authorization = Cookies.get("access-token");

    const [form] = Form.useForm();

    const onFinish = (values) => {

        const user = {
            Name: values.name,
            Email: values.email,
            Role: values.rol,
            password: values.password,
            PasswordConfirmation: values.confirm
        }

        axios.post(`${Config.API_URL}/User/create-user`, user, {
            //axios.post(`${API_URL}/User/create-user`, user, {
            headers: { Authorization: "Bearer " + authorization },
        }).then(res => {
        }).catch(err => {
        }).finally(() => {
            form.resetFields();
            setLoading(false)
            setModalAddUserVisible(false);
        });

    }

    const handleCancel = () => {
        form.resetFields();
        setModalAddUserVisible(false);
    }


    useEffect(() => {
    }, [isModalAddUserVisible])


    return (
        <Modal
            style={{ right: 55 }}
            title={"Agregar Usuario"}
            visible={isModalAddUserVisible}
            onCancel={handleCancel}
            footer={[
                <Button key="back" onClick={handleCancel}>
                    Cancelar
                </Button>,
                <Button key="submit" className='btn-primary' type="primary" onClick={() => form.submit()} loading={loading}>
                    {!loading ? "Guardar" : "Cargando..."}
                </Button>,
            ]}
        >
            <Form
                layout="vertical"
                widht='100%'
                form={form}
                onFinish={onFinish}
            >
                <Row>
                    <Col span={24}>
                        <Form.Item label="Nombre Completo" style={{ marginBottom: '0px' }}>
                            <Input.Group>
                                <Form.Item
                                    align='left'
                                    name='name'
                                    rules={[
                                        { required: true, message: "Por favor ingrese su nombre completo!" },
                                    ]}
                                    style={{ width: '100%' }}
                                >
                                    <Input
                                        style={{ width: '100%' }}
                                        placeholder='Jhon Smith'
                                        name='name'
                                    />
                                </Form.Item>
                            </Input.Group>
                        </Form.Item>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <Form.Item label="Correo" style={{ marginBottom: '0px' }}>
                            <Input.Group>
                                <Form.Item
                                    align='left'
                                    name='email'
                                    rules={[
                                        { required: true, message: "Por favor ingrese su correo!" },
                                        { pattern: /^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$/g, message: "Por favor un correo válido!" },
                                    ]}
                                    style={{ width: '100%' }}
                                >
                                    <Input
                                        style={{ width: '100%' }}
                                        placeholder='jhon.smith@correo.com'
                                        name='email'
                                        type='email'
                                    />
                                </Form.Item>
                            </Input.Group>
                        </Form.Item>
                    </Col>
                </Row>
                <Row>
                    <Col span={24}>
                        <Form.Item label="Rol" style={{ marginBottom: '-10px' }}>
                            <Input.Group>
                                <Form.Item
                                    name='rol'
                                    rules={[{ required: true, message: 'Tipo de rol.' }]}
                                    style={{ width: '100%' }}
                                >
                                    <Select placeholder="Rol" style={{ width: '100%' }} name='rol'>
                                        <Option value="Agent">Agente</Option>
                                        <Option value="Admin">Administrador</Option>
                                    </Select>
                                </Form.Item>
                            </Input.Group>
                        </Form.Item>
                    </Col>
                </Row>
          <Row>
            <Col span={24}>
              <Form.Item
                name="password"
                label="Contraseña"
                rules={[
                  {
                    required: true,
                    message: 'Por favor, introduzca su contraseña.',
                  },
                  {
                    pattern: /^(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[a-zA-Z]).{8,}$/gm,
                    message: "La contraseña no cumple las políticas establecidas",
                  },
                ]}
                hasFeedback
                style={{marginBottom: '10px'}}
              >
                <Input.Password />
              </Form.Item>

              <Form.Item
                name="confirm"
                label="Confirmar contraseña"
                dependencies={['password']}
                hasFeedback
                rules={[
                  {
                    required: true,
                    message: 'Por favor, confirme su contraseña.',
                  },
                  ({ getFieldValue }) => ({
                    validator(_, value) {
                      if (!value || getFieldValue('password') === value) {
                        return Promise.resolve();
                      }
                      return Promise.reject(new Error('Las dos contraseñas que has introducido no coinciden.'));
                    },
                  }),
                ]}
                style={{marginBottom: '10px'}}
                >
                <Input.Password />
              </Form.Item>
                <div style={{ textAlign: 'left', padding: '-10px 0 0 0' }}>
                    <p style={{ fontSize: '12px' }}>
                        * La contraseña debe tener al menos una mayúscula <br />
                        * La contraseña debe tener al menos una minúscula <br />
                        * La contraseña debe tener al menos un número <br />
                        * La contraseña debe tener al menos 8 caracteres <br />
                    </p>
                </div>
            </Col>
          </Row>
            </Form>
        </Modal>
    )
}