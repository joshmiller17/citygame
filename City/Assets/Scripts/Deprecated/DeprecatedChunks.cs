
// From PlayerController:
//void Move()
//{
    //float h = Input.GetAxis("Horizontal");
    //float v = Input.GetAxis("Vertical");
    //float yStore = moveDirection.y;

    //moveDirection = (transform.forward * v) + (transform.right * h);
    //moveDirection = moveDirection.normalized * moveSpeed * controller.m_MoveSpeedMultiplier;
    //if (boostDuration > 0)
    //{
    //    moveDirection *= (1 + speedBoost);
    //}
    //moveDirection.y = yStore;

    //if (controller.isGrounded)
    //{
    //    TimeSinceLastGround = 0;
    //    moveDirection.y = 0;
    //}
    //else
    //{
    //    TimeSinceLastGround += Time.deltaTime;
    //    jumpAmountLeft = Mathf.Max(jumpAmountLeft - Time.deltaTime, 0);
    //}


    //if (TimeSinceLastGround < CoyoteTime && Input.GetButton("Jump"))
    //{
    //    if (!isJumping)
    //    {
    //        jumpAmountLeft = jumpDuration + ((controller.m_MoveSpeedMultiplier - 1) * 0.1f);
    //    }
    //    isJumping = true;
    //}
    //if (Input.GetButtonUp("Jump") || jumpAmountLeft <= 0)
    //{
    //    isJumping = false;
    //}

    //if (isJumping)
    //{
    //    moveDirection.y = jumpForce + Mathf.Pow(controller.m_MoveSpeedMultiplier, 1.01f);
    //    if (boostDuration > 0)
    //    {
    //        moveDirection.y *= (1 + jumpMultiplier);
    //    }
    //    //gameObject.transform.Find("R_arm").gameObject //TODO
    //}
    //else
    //{
    //    moveDirection.y = moveDirection.y + (gravityScale * Physics.gravity.y);
    //}

    //controller.Move(moveDirection * Time.deltaTime);

    //if (h != 0 || v != 0)
    //{
    //    //transform.rotation = Quaternion.Euler(0f, pivot.rotation.eulerAngles.y, 0f);
    //    //Quaternion newRotation = Quaternion.LookRotation(new Vector3(moveDirection.x, 0f, moveDirection.z));

    //    //FIXME this causes jitter when walking backwards
    //    //transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, rotateSpeed * Time.deltaTime); //TODO rotate the player model, not the transform
    //}

    //if (!IsMovingFast() || !MusicOn) //slow down if we're not moving and listening to music
    //{
    //    controller.m_MoveSpeedMultiplier = Mathf.Max(controller.m_MoveSpeedMultiplier * 0.999f, 1);
    //}

//}