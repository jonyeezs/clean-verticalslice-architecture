# Remove this when when we have a proper DNS
resource "aws_acm_certificate" "temporary" {
  domain_name       = "*.amazonaws.com"
  validation_method = "DNS"

  tags = {
    Environment = "Temporary"
  }

  lifecycle {
    create_before_destroy = true
  }
}

resource "aws_lb" "this" {
  name            = "${local.service}-alb"
  security_groups = [aws_security_group.this.id]
  subnets         = data.aws_subnets.this.ids
}

resource "aws_lb_listener" "this" {
  load_balancer_arn = aws_lb.this.arn

  port            = "443"
  protocol        = "HTTPS"
  ssl_policy      = "ELBSecurityPolicy-2016-08"
  certificate_arn = aws_acm_certificate.temporary.arn

  default_action {
    type = "fixed-response"

    fixed_response {
      content_type = "text/plain"
      message_body = "Sorry, we could not find the resource you were looking for. Please check your request and try again."
      status_code  = "404"
    }
  }
}

resource "aws_lb_target_group" "this" {
  name        = "${local.service}-lb-tg"
  port        = 80
  protocol    = "HTTP"
  target_type = "ip"
  vpc_id      = data.aws_vpc.this.id

  health_check {
    path                = "/health-probe"
    protocol            = "HTTPS"
    port                = "traffic-port"
    interval            = 30
    timeout             = 10
    healthy_threshold   = 3
    unhealthy_threshold = 3
  }
}

resource "aws_lb_listener_rule" "this" {
  listener_arn = aws_lb_listener.this.arn

  action {
    type             = "forward"
    target_group_arn = aws_lb_target_group.this.arn
  }

  condition {
    path_pattern {
      values = ["/api/*"]
    }
  }
}
