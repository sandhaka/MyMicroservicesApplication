FROM fluent/fluentd:v0.14-debian

RUN ["gem", "install", "fluent-plugin-elasticsearch", "--no-rdoc", "--no-ri", "--version", "1.10.0"]

COPY fluent.conf /fluentd/etc/